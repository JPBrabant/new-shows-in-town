"""Azure Function (v2): timer-triggered scraper for Salle André-Mathieu.

Runs on a schedule, scrapes all shows, and syncs them to the NewShowsInTown API.
"""

import logging
import os

import azure.functions as func

from main import scrape_shows, sync_to_api

app = func.FunctionApp()
API_BASE = os.environ.get("NEWSHOWS_API_BASE", "https://localhost:5001")


@app.timer_trigger(
    schedule="0 0 */6 * * *",
    arg_name="timer",
    run_on_startup=False,
)
def scrape_salle_andre_mathieu(timer: func.TimerRequest) -> None:
    logging.info("SalleAndreMathieu: starting scrape run")

    try:
        shows = scrape_shows()
        logging.info("Scraped %d shows from Salle André-Mathieu", len(shows))
    except Exception:
        logging.exception("Scraping failed")
        raise

    try:
        result = sync_to_api(shows)
        logging.info(
            "Sync complete: %d created, %d updated, %d failed",
            result["created"], result["updated"], result["failed"],
        )
    except Exception:
        logging.exception("API sync failed")
        raise
