import { test, expect } from "@playwright/test";

test("clerk creates an order and adds a product", async ({ page }) => {
  await page.goto("/");

  const idsBefore = await page.$$eval("#orders-list .order", (els) =>
    els.map((el) => (el as HTMLElement).dataset.orderId),
  );

  await page.getByRole("button", { name: "Create new order" }).click();

  await expect
    .poll(async () =>
      page.$$eval("#orders-list .order", (els) => els.length),
    )
    .toBe(idsBefore.length + 1);

  const newOrderId = await page.$$eval(
    "#orders-list .order",
    (els, before) =>
      (els as HTMLElement[])
        .map((el) => el.dataset.orderId!)
        .find((id) => !before.includes(id)),
    idsBefore,
  );
  expect(newOrderId).toBeTruthy();

  await page.selectOption("#product-order", newOrderId!);
  await page.fill("#product-name", "Wine Glass");
  await page.selectOption("#product-color", "Red");
  await page.selectOption("#product-size", "Small");
  await page.fill("#product-price", "12.50");
  await page.fill("#product-discount", "0.20");
  await page.selectOption("#product-material", "Glass");
  await page.fill("#product-weight", "0.3");
  await page.selectOption("#product-packaging", "Boxed");
  await page.check("#product-fragility");
  await page.fill("#product-length", "8");
  await page.fill("#product-width", "8");
  await page.fill("#product-height", "20");

  await page.getByRole("button", { name: "Add product" }).click();

  const order = page.locator(`.order[data-order-id="${newOrderId}"]`);
  await expect(order.locator(".product")).toContainText("Wine Glass");
  await expect(order.locator(".order-header")).toContainText("total $10.00");
});
