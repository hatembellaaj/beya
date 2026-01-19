from tests.ui import config
from tests.ui.pages.cart_page import CartPage
from tests.ui.pages.login_page import LoginPage
from tests.ui.pages.order_page import OrderPage


def test_cart_to_order_flow(driver):
    login_page = LoginPage(driver, config.BASE_URL)
    login_page.load()
    login_page.login(config.USERNAME, config.PASSWORD)

    cart_page = CartPage(driver, config.BASE_URL)
    cart_page.load()
    assert cart_page.has_items()
    cart_page.checkout()

    order_page = OrderPage(driver, config.BASE_URL)
    order_page.confirm_order()
    assert order_page.status_text()
