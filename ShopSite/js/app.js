// Код сайта. Здесь — что происходит, когда пользователь что-то нажал или выбрал.
// Элементы страницы описаны в index.html, здесь мы находим их по id.

const productsList = document.getElementById('products');
const categorySelect = document.getElementById('category');
const searchInput = document.getElementById('search');
const cartList = document.getElementById('cart');
const totalText = document.getElementById('total');
const clientSelect = document.getElementById('client');
const orderButton = document.getElementById('order-button');
const messageText = document.getElementById('message');

// Все товары, которые пришли из API. Фильтры выбирают товары из этого списка
let allProducts = [];

// Корзина — список строк. Каждая строка: { productId, name, price, quantity }
let cart = [];

// Показать сообщение под кнопкой «Оформить заказ»
function showMessage(text) {
    messageText.textContent = text;
}

// ============ Загрузка данных. Готово, ошибок нет ============

// Добавляем категории в выпадающий список. Номер категории — в value, название — в тексте
async function loadCategories() {
    const categories = await getCategories();
    for (const category of categories) {
        const option = document.createElement('option');
        option.value = category.id;
        option.textContent = category.name;
        categorySelect.append(option);
    }
}

// Добавляем клиентов в выпадающий список так же, как категории
async function loadClients() {
    const clients = await getClients();
    for (const client of clients) {
        const option = document.createElement('option');
        option.value = client.id;
        option.textContent = client.name + ', ' + client.city;
        clientSelect.append(option);
    }
}

// Загружаем товары из API и показываем их с учётом фильтров
async function loadProducts() {
    allProducts = await getProducts();
    applyFilter();
}

// ============ Часть A. Каталог ============

// Выбираем из всех товаров те, что подходят под фильтры, и показываем их
function applyFilter() {
    let products = allProducts;

    // Фильтр 1: категория. Пустое значение — это «Все категории»
    const categoryId = categorySelect.value;
    if (categoryId !== '') {
        // Подсказка: p.categoryId это число, а categoryId это строка. Строка никогда не будет равна числу.
        products = products.filter(p => p.categoryId === Number(categoryId));
    }

    // Фильтр 2: поиск по названию (задание 4)
    const searchText = searchInput.value.trim().toLowerCase();
    if (searchText !== '') {
        products = products.filter(p => p.name.toLowerCase().includes(searchText));
    }

    renderProducts(products);
}

// Рисуем карточки товаров
function renderProducts(products) {
    productsList.innerHTML = '';

    for (const product of products) {
        const card = document.createElement('div');
        card.className = 'card';

        // Товара нет на складе — карточка с классом out, кнопка не нажимается
        let disabled = '';
        if (product.stock === 0) {
            card.classList.add('out');
            disabled = 'disabled';
        }

        // Подсказка: Карточка товара генерируется здесь. Что то не так с ценой
        card.innerHTML = `
            <div class="card-id">№ ${product.id} ₽</div>
            <h3>${product.name}</h3>
            <div class="price">${product.price} ₽</div>
            <div class="stock">На складе: ${product.stock} шт.</div>
            <button ${disabled}>В корзину</button>
        `;

        card.querySelector('button').addEventListener('click', () => addToCart(product));
        productsList.append(card);
    }
}

// ============ Часть B. Корзина ============

// Кнопка «В корзину». Товар уже в корзине — добавляем ещё штуку, нет — новую строку
function addToCart(product) {
    const line = cart.find(l => l.productId === product.id);

    if (line) {
        // Подсказка: Количество не прибавляется здесь.
        line.quantity += 1;
    } else {
        cart.push({
            productId: product.id,
            name: product.name,
            price: product.price,
            quantity: 1,
        });
    }

    renderCart();
}

// Кнопка ✕ в корзине — убираем строку с этим товаром
function removeFromCart(productId) {
    // Подсказка: Когда мы убираем продукт из корзины, как нам нужно фильтровать?
    cart = cart.filter(l => l.productId !== productId);
    renderCart();
}

// Рисуем корзину
function renderCart() {
    cartList.innerHTML = '';

    if (cart.length === 0) {
        cartList.innerHTML = '<li class="empty">Корзина пуста</li>';
    }

    for (const line of cart) {
        const li = document.createElement('li');
        li.innerHTML = `
            <span>${line.name}</span>
            <span>${line.quantity} × ${line.price} ₽</span>
            <button class="remove" title="Убрать">✕</button>
        `;
        li.querySelector('button').addEventListener('click', () => removeFromCart(line.productId));
        cartList.append(li);
    }

    // Сумма корзины (задание 7)

    let total = 0;
    for (const line of cart) {
        total += line.price * line.quantity;
    }
    // Подсказка: Добавь сюда подсчёт цены для всех товаров
    totalText.textContent = `Итого: ${total} ₽`;
}

// ============ Часть C. Заказ ============

// Кнопка «Оформить заказ» (задание 8)
async function makeOrder() {
    const clientId = clientSelect.value;
    if (clientId === '') {
        showMessage('Выбери клиента');
        return;
    }

    if (cart.lenght === 0) {
        showMessage('Корзина пуста');
        return;
    }
    const result = await createOrder(Number(clientId), cart);
    if (result.error) {
        showMessage(result.error);
        return;
    }

    showMessage(`Заказ №${result.order.id} оформлен`);
    cart = [];
    renderCart();
    await loadProducts();

}

// ============ Запуск ============

// Связываем элементы страницы с функциями: что вызывать, когда пользователь что-то сделал
categorySelect.addEventListener('change', applyFilter);
searchInput.addEventListener('input', applyFilter);
orderButton.addEventListener('click', makeOrder);

// Страница открылась — загружаем данные из API
async function start() {
    renderCart();
    try {
        await loadCategories();
        await loadClients();
        await loadProducts();
    } catch (error) {
        console.error(error);
        showMessage('Не получилось связаться с API. Запусти ShopAPI (README, шаг 2) и обнови страницу');
    }
}

start();
