const ordersList = document.getElementById("orders-list");
const orderSelect = document.getElementById("product-order");

async function loadOrders() {
  const response = await fetch("/api/orders");
  const orders = await response.json();
  renderOrders(orders);
  renderOrderOptions(orders);
}

function renderOrders(orders) {
  ordersList.innerHTML = "";

  if (orders.length === 0) {
    const empty = document.createElement("li");
    empty.className = "empty-state";
    empty.textContent = "No orders yet. Create one above.";
    ordersList.appendChild(empty);
    return;
  }

  orders.forEach((order, index) => {
    const li = document.createElement("li");
    li.className = "order";
    li.dataset.orderId = order.id;

    const header = document.createElement("div");
    header.className = "order-header";

    const title = document.createElement("span");
    title.className = "order-title";
    title.textContent = `Order #${index + 1} — total $${order.total.toFixed(2)}`;
    header.appendChild(title);

    const badge = document.createElement("span");
    badge.className = `status-badge status-${order.status.toLowerCase()}`;
    badge.textContent = order.status;
    header.appendChild(badge);

    li.appendChild(header);

    if (order.status === "Pending") {
      const confirmBtn = document.createElement("button");
      confirmBtn.className = "btn-confirm confirm-order";
      confirmBtn.textContent = "Confirm order";
      confirmBtn.addEventListener("click", async () => {
        await fetch(`/api/orders/${order.id}/confirm`, { method: "POST" });
        await loadOrders();
      });
      li.appendChild(confirmBtn);
    }

    const products = order.products ?? [];
    if (products.length > 0) {
      const productList = document.createElement("ul");
      productList.className = "products";
      for (const product of products) {
        const item = document.createElement("li");
        item.className = "product";
        item.textContent =
          `${product.name} (${product.color}, ${product.size}) — ` +
          `$${product.price.toFixed(2)} @ ${(product.discount * 100).toFixed(0)}% off`;
        productList.appendChild(item);
      }
      li.appendChild(productList);
    }

    ordersList.appendChild(li);
  });
}

function renderOrderOptions(orders) {
  const previous = orderSelect.value;
  orderSelect.innerHTML = "";
  orders.forEach((order, index) => {
    const option = document.createElement("option");
    option.value = order.id;
    option.textContent = `Order #${index + 1} (${order.status})`;
    orderSelect.appendChild(option);
  });
  if (previous) orderSelect.value = previous;
}

document.getElementById("create-order").addEventListener("click", async () => {
  await fetch("/api/orders", { method: "POST" });
  await loadOrders();
});

document.getElementById("add-product-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const orderId = orderSelect.value;
  if (!orderId) return;

  const product = {
    name: document.getElementById("product-name").value,
    color: document.getElementById("product-color").value,
    size: document.getElementById("product-size").value,
    price: parseFloat(document.getElementById("product-price").value),
    discount: parseFloat(document.getElementById("product-discount").value),
    material: document.getElementById("product-material").value,
    weight: parseFloat(document.getElementById("product-weight").value),
    fragility: document.getElementById("product-fragility").checked,
    containsLiquids: document.getElementById("product-contains-liquids").checked,
    packaging: document.getElementById("product-packaging").value,
    dimensions: {
      length: parseFloat(document.getElementById("product-length").value),
      width: parseFloat(document.getElementById("product-width").value),
      height: parseFloat(document.getElementById("product-height").value),
    },
  };

  await fetch(`/api/orders/${orderId}/products`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(product),
  });

  event.target.reset();
  document.getElementById("product-discount").value = "0";
  await loadOrders();
});

loadOrders();
