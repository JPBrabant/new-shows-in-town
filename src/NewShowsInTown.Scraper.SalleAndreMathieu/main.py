import httpx
from bs4 import BeautifulSoup

BASE_URL = "https://co-motion.ca/spectacles/?salle=salle-andre-mathieu"


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