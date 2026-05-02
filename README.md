# new-shows-in-town
A .NET web app that aggregates local shows (movies, theater, concerts) from multiple venues.

Fetch all shows and movies from 
- Salle André-Mathieu (co-motion)
    - Use BeautifulSoup4 to parse this [web page](https://co-motion.ca/spectacles/)
- Place Bell (evenko)
    - Do a GET at this URL `https://placebell.ca/api/algolia/search?query=abc123` where abc123 is a base64 encoded Algolia query
    ```json
    {
        "filters": {
            "vue": "grille",
            "displayMode": "grid",
            "type": ["evenko_show","show"],
            "search": ""
        },
        "options": {
            "hitsPerPage": 20,
            "page": 0
        },
        "indexName": "master_evenko_en-CA",
        "venueSlug": "place-bell"
    }
    ```
- Cineplex
    - TODO