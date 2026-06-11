function initDragAndDrop() {
    const list = document.getElementById('task-list');
    if (!list) return;

    let dragging = null;

    list.querySelectorAll('.task-card').forEach(card => {
        card.addEventListener('dragstart', () => {
            dragging = card;
            setTimeout(() => card.classList.add('dragging'), 0);
        });
        card.addEventListener('dragend', () => {
            card.classList.remove('dragging');
            dragging = null;
            saveOrder();
        });
    });

    list.addEventListener('dragover', e => {
        e.preventDefault();
        const after = getDragAfterElement(list, e.clientY);
        if (after == null) list.appendChild(dragging);
        else list.insertBefore(dragging, after);
    });

    function getDragAfterElement(container, y) {
        const cards = [...container.querySelectorAll('.task-card:not(.dragging)')];
        return cards.reduce((closest, child) => {
            const box = child.getBoundingClientRect();
            const offset = y - box.top - box.height / 2;
            return offset < 0 && offset > closest.offset ? { offset, element: child } : closest;
        }, { offset: Number.NEGATIVE_INFINITY }).element;
    }

    function saveOrder() {
        const ids = [...list.querySelectorAll('.task-card')].map(c => parseInt(c.dataset.id));
        fetch('/Todo/Reorder', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(ids)
        });
    }
}

// Live search on keyup
document.addEventListener('DOMContentLoaded', () => {
    const searchInput = document.querySelector('.search-input');
    if (searchInput) {
        let timer;
        searchInput.addEventListener('input', () => {
            clearTimeout(timer);
            timer = setTimeout(() => searchInput.closest('form').submit(), 400);
        });
    }
});