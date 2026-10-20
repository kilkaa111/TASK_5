// Всё общение с API — здесь. Этот файл уже готов, менять его не нужно.
//
// Как пользоваться (не забывай слово await!):
//     const products = await getProducts();
//     const result = await createOrder(clientId, cart);

// Адрес, на котором работает ShopAPI (README, шаг 2)
const API_URL = 'http://localhost:5000';

// GET /categories — все категории
async function getCategories() {
    const response = await fetch(API_URL + '/categories');
    return await response.json();
}

// GET /products — все товары
async function getProducts() {
    const response = await fetch(API_URL + '/products');
    return await response.json();
}

// GET /clients — все клиенты
async function getClients() {
    const response = await fetch(API_URL + '/clients');
    return await response.json();
}

// POST /orders?clientId=... — оформить заказ.
// cart — строки корзины, у каждой должны быть productId и quantity.
// Возвращает { order: созданный заказ } или { error: 'текст ошибки' }
async function createOrder(clientId, cart) {
    const response = await fetch(API_URL + '/orders?clientId=' + clientId, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(cart),
    });

    if (!response.ok) {
        const text = await response.text();
        if (text === '' || text.startsWith('{')) {
            return { error: 'Ошибка ' + response.status };
        }
        return { error: text };
    }

    return { order: await response.json() };
}
