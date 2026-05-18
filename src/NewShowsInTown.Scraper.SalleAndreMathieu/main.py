import httpx
from bs4 import BeautifulSoup

BASE_URL = "https://co-motion.ca/spectacles/?salle=salle-andre-mathieu"

def main():
    page = 1
    max_page = None
    
    # Construct URL with pagination
    url = f"{BASE_URL}&page={page}"
    
    # Fetch the page
    response = httpx.get(url)
    response.raise_for_status()
    
    # Parse with BeautifulSoup
    soup = BeautifulSoup(response.text, "html.parser")
    
    max_page = max(int(a.get_text(strip=True)) for a in soup.find_all("a", class_="pagination__button"))

    print(max_page)


if __name__ == "__main__":
    main()
