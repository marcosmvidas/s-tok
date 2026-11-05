(() => {
    // Menu lateral responsivo
    const toggle = document.getElementById("menuToggle");
    const overlay = document.getElementById("sidebarOverlay");

    if (toggle && overlay) {
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
    }

    // Modo formulário: bloqueia a navegação por mouse e teclado para o restante da página.
    const formularioProtegido = document.getElementById("formProtectedOverlay");

    if (!formularioProtegido) {
        return;
    }

    document.body.classList.add("form-protected-active");

    const elementosFocaveis = () => Array.from(
        formularioProtegido.querySelectorAll(
            'a[href], button:not([disabled]), input:not([disabled]):not([type="hidden"]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])'
        )
    ).filter(elemento => elemento.getClientRects().length > 0);

    // Coloca o foco dentro do formulário ao abrir a página.
    const primeiroElemento = elementosFocaveis()[0];
    if (primeiroElemento) {
        primeiroElemento.focus();
    } else {
        formularioProtegido.focus();
    }

    document.addEventListener("keydown", event => {
        if (event.key === "Escape") {
            // Não permite fechar o modo formulário com Escape.
            event.preventDefault();
            event.stopPropagation();
            return;
        }

        if (event.key !== "Tab") {
            return;
        }

        const focaveis = elementosFocaveis();

        if (focaveis.length === 0) {
            event.preventDefault();
            formularioProtegido.focus();
            return;
        }

        const primeiro = focaveis[0];
        const ultimo = focaveis[focaveis.length - 1];

        if (event.shiftKey && document.activeElement === primeiro) {
            event.preventDefault();
            ultimo.focus();
        } else if (!event.shiftKey && document.activeElement === ultimo) {
            event.preventDefault();
            primeiro.focus();
        } else if (!formularioProtegido.contains(document.activeElement)) {
            event.preventDefault();
            primeiro.focus();
        }
    });
})();
