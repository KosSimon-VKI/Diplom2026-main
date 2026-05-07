let activeSubcategory = null;

// ===== DOM READY =====
document.addEventListener("DOMContentLoaded", () => {
    initCategoryButtons();
    initSearch();
    initPriceFilter();
    initMobileFilters();
    updateSubcategories("all");
    applyFilters();
});

// ===== CATEGORY FILTER =====
function initCategoryButtons() {
    document.querySelectorAll('.main-category-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            document.querySelectorAll('.main-category-btn')
                .forEach(b => b.classList.remove('active'));

            this.classList.add('active');
            activeSubcategory = null;

            const type = this.dataset.type;
            updateSubcategories(type);
            applyFilters();
        });
    });
}

// ===== SEARCH =====
function initSearch() {
    const searchInput = document.getElementById('search-input');
    const mobileSearchInput = document.getElementById('mobile-search-input');

    searchInput?.addEventListener('input', e => {
        if (mobileSearchInput && mobileSearchInput.value !== e.target.value) {
            mobileSearchInput.value = e.target.value;
        }

        applyFilters();
    });

    mobileSearchInput?.addEventListener('input', e => {
        if (searchInput && searchInput.value !== e.target.value) {
            searchInput.value = e.target.value;
        }

        applyFilters();
    });
}

// ===== PRICE FILTER =====
function initPriceFilter() {
    const priceFilter = document.getElementById('price-filter');
    const priceValue = document.getElementById('price-value');

    priceFilter?.addEventListener('input', () => {
        priceValue.textContent = priceFilter.value;
        applyFilters();
    });
}

// ===== MOBILE FILTER PANEL =====
function initMobileFilters() {
    const filters = document.querySelector('.filters');
    const backdrop = document.getElementById('filters-backdrop');
    const openButton = document.getElementById('mobile-filter-open');
    const closeButton = document.getElementById('mobile-filter-close');

    openButton?.addEventListener('click', () => {
        filters?.classList.add('filters-open');
        backdrop?.classList.remove('hidden');
    });

    const closeFilters = () => {
        filters?.classList.remove('filters-open');
        backdrop?.classList.add('hidden');
    };

    closeButton?.addEventListener('click', closeFilters);
    backdrop?.addEventListener('click', closeFilters);
}

// ===== FILTER LOGIC =====
function applyFilters() {
    const type = document.querySelector('.main-category-btn.active')?.dataset.type ?? 'all';
    const searchTerm = (document.getElementById('search-input')?.value ?? '').trim().toLowerCase();
    const maxPrice = parseInt(document.getElementById('price-filter')?.value ?? '1000');

    document.querySelectorAll('.item-card').forEach(item => {
        const itemType = item.dataset.type;
        const itemCategory = item.dataset.category ?? '';
        const name = item.querySelector('.item-name')?.textContent.toLowerCase() ?? '';
        const category = item.querySelector('.item-category')?.textContent.toLowerCase() ?? '';
        const price = parseFloat(item.querySelector('.item-price')?.textContent) || 0;

        const matchesType = type === 'all' || itemType === type;
        const matchesSubcategory = !activeSubcategory || itemCategory === activeSubcategory;
        const matchesSearch = !searchTerm || name.includes(searchTerm) || category.includes(searchTerm);
        const matchesPrice = price <= maxPrice;

        item.style.display = (matchesType && matchesSubcategory && matchesSearch && matchesPrice)
            ? 'flex'
            : 'none';
    });
}

// ===== SUBCATEGORIES =====
function updateSubcategories(type) {
    const container = document.getElementById('subcategories');
    container.innerHTML = '';

    if (type === 'all') return;

    const items = document.querySelectorAll(`.item-card[data-type="${type}"]`);
    const categories = new Set();

    items.forEach(i => {
        if (i.dataset.category) {
            categories.add(i.dataset.category);
        }
    });

    if (categories.size === 0) return;

    let html = '<div class="subcategory-title">Подкатегории:</div><div class="subcategory-buttons">';
    categories.forEach(c => {
        html += `<button class="subcategory-btn" data-category="${c}">${c}</button>`;
    });
    html += '</div>';

    container.innerHTML = html;

    document.querySelectorAll('.subcategory-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            const isActive = btn.classList.contains('active');

            document.querySelectorAll('.subcategory-btn')
                .forEach(b => b.classList.remove('active'));

            activeSubcategory = isActive ? null : btn.dataset.category;

            if (activeSubcategory) {
                btn.classList.add('active');
            }

            applyFilters();
        });
    });
}
