from tests.ui import config
from tests.ui.pages.login_page import LoginPage


def test_login_flow(driver):
    login_page = LoginPage(driver, config.BASE_URL)
    login_page.load()
    login_page.login(config.USERNAME, config.PASSWORD)
    assert "/" in driver.current_url
