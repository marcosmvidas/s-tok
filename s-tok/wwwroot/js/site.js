javascript
(() => {
    const toggle = document.getElementById("menuToggle");
    const overlay = document.getElementById("sidebarOverlay");

    if (!toggle || !overlay) {
        return;
    }

    function setMenuOpen(isOpen) {
        document.body.classList.toggle("sidebar-open", isOpen);
        toggle.setAttribute("aria-expanded", String(isOpen));
    }

    toggle.addEventListener("click", () => {
        const isOpen = document.body.classList.contains("sidebar-open");
        setMenuOpen(!isOpen);
    });

    overlay.addEventListener("click", () => {
        setMenuOpen(false);
    });

    document.addEventListener("keydown", event => {
        if (event.key === "Escape") {
            setMenuOpen(false);
        }
    });
})();
