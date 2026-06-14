import datetime
import logging
import os

import httpx
from bs4 import BeautifulSoup

BASE_URL = "https://co-motion.ca/spectacles/?salle=salle-andre-mathieu"
VENUE_ID = 1  # Salle André-Mathieu in the database
API_BASE = os.environ.get("NEWSHOWS_API_BASE", "https://localhost:5001")

FRENCH_MONTHS = {
    "janvier": 1, "février": 2, "fevrier": 2,
    "mars": 3, "avril": 4, "mai": 5, "juin": 6,
    "juillet": 7, "août": 8, "aout": 8,
    "septembre": 9, "octobre": 10,
    "novembre": 11, "décembre": 12, "decembre": 12,
}


def scrape_shows() -> list[dict]:
    """Scrape all shows from Salle André-Mathieu and return as a list of dicts.

    Returns:
        List of dicts with keys: title, subtitle, date, img.
    """
    all_shows: list[dict] = []

    with httpx.Client(timeout=30) as client:
        response = client.get(BASE_URL)
        response.raise_for_status()
        html = BeautifulSoup(response.text, "html.parser")

        max_page = max(
            int(a.get_text(strip=True))
            for a in html.find_all("a", class_="pagination__button")
        )

        for page in range(1, max_page + 1):
            if page > 1:
                url = f"{BASE_URL}&page={page}"
                response = client.get(url)
                response.raise_for_status()
                html = BeautifulSoup(response.text, "html.parser")

            for card in html.find_all("a", class_="ShowCard"):
                all_shows.append(extract_show_info(card))

    return all_shows


def extract_show_info(card):
    title_tag = card.find("h3", class_="ShowCard__header__content__title")
    subtitle_tag = card.find("h4", class_="ShowCard__header__content__subtitle")
    date_tag = card.find("h4", class_="ShowCard__header__content__date")
    img_tag = card.find("img")

    return {
        "title": title_tag.get_text(strip=True) if title_tag else None,
        "subtitle": subtitle_tag.get_text(strip=True)[2:] if subtitle_tag else None,
        "date": date_tag.get_text(strip=True) if date_tag else None,
        "img": img_tag["src"] if img_tag and img_tag.has_attr("src") else None,
    }


def parse_french_date(date_str: str) -> str | None:
    """Convert French date like '16 juin 2026' to ISO 8601 string."""
    try:
        parts = date_str.strip().split()
        day = int(parts[0])
        month_name = parts[1].lower().replace("û", "u").replace("é", "e").replace("è", "e")
        month = FRENCH_MONTHS[month_name]
        year = int(parts[2])
        dt = datetime.datetime(year, month, day, 0, 0, 0)
        return dt.isoformat()
    except (ValueError, KeyError, IndexError):
        logging.warning("Could not parse date: %s", date_str)
        return None


def sync_to_api(shows: list[dict]) -> dict[str, int]:
    """POST each show to the upsert endpoint. Returns counts of created/updated/failed."""
    created = 0
    updated = 0
    failed = 0

    with httpx.Client(timeout=30) as client:
        for show in shows:
            iso_date = parse_french_date(show["date"]) if show["date"] else None
            if not iso_date:
                failed += 1
                continue

            payload = {
                "title": show["title"],
                "language": "French",
                "venueId": VENUE_ID,
                "subtitle": show.get("subtitle"),
                "imageUrl": show.get("img"),
                "description": None,
                "eventUrl": None,
                "category": None,
                "showTimes": [{"startsAt": iso_date}],
            }

            try:
                response = client.post(
                    f"{API_BASE}/shows/upsert",
                    json=payload,
                )
                if response.status_code in (200, 201):
                    if response.status_code == 201:
                        created += 1
                    else:
                        updated += 1
                else:
                    logging.warning(
                        "API error for '%s': %s %s",
                        show["title"], response.status_code, response.text,
                    )
                    failed += 1
            except httpx.RequestError:
                logging.exception("Request failed for '%s'", show["title"])
                failed += 1

    return {"created": created, "updated": updated, "failed": failed}


def main():
    """CLI entry point: scrape and print all shows."""
    shows = scrape_shows()

    print(f"\n{'='*60}")
    print(f"  TOTAL SHOWS: {len(shows)}")
    print(f"{'='*60}\n")

    for i, show in enumerate(shows, 1):
        print(f"  #{i:2d}  {show['title']}")
        if show["subtitle"]:
            print(f"       └─ {show['subtitle']}")
        if show["date"]:
            print(f"       └─ {show['date']}")
        print()


if __name__ == "__main__":
    main()