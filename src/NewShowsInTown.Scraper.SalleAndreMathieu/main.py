import httpx
from bs4 import BeautifulSoup

BASE_URL = "https://co-motion.ca/spectacles/?salle=salle-andre-mathieu"

def main():
    # Construct URL with pagination
    #url = f"{BASE_URL}&page={page}"
    
    # Fetch the page
    response = httpx.get(BASE_URL)
    response.raise_for_status()
    
    # Parse with BeautifulSoup
    html = BeautifulSoup(response.text, "html.parser")

    # Determine the maximum page number for pagination
    max_page = max(int(a.get_text(strip=True)) for a in html.find_all("a", class_="pagination__button"))

    show_cards = html.find_all("a", class_="ShowCard")
    for card in show_cards:
        info = extract_show_info(card)
        print(info)

def extract_show_info(card):
    title_tag    = card.find("h3", class_="ShowCard__header__content__title")
    subtitle_tag = card.find("h4", class_="ShowCard__header__content__subtitle")
    date_tag     = card.find("h4", class_="ShowCard__header__content__date")
    img_tag      = card.find("img")

    return {
        "title":    title_tag.get_text(strip=True) if title_tag else None,
        "subtitle": subtitle_tag.get_text(strip=True)[2:] if subtitle_tag else None,
        "date":     date_tag.get_text(strip=True) if date_tag else None,
        "img":      img_tag["src"] if img_tag and img_tag.has_attr("src") else None,
    }

if __name__ == "__main__":
    main()