
(() => {
    // MENU LATERAL
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

    // PROTEÇÃO CONTRA PERDA DE ALTERAÇÕES
    const formulario = document.querySelector(
        "form[data-protect-unsaved]"
    );

    if (!formulario) {
        return;
    }

    let formularioAlterado = false;
    let formularioEnviado = false;

    formulario.addEventListener("input", () => {
        formularioAlterado = true;
    });

    formulario.addEventListener("change", () => {
        formularioAlterado = true;
    });

    formulario.addEventListener("submit", () => {
        formularioEnviado = true;
    });

    // Protege a navegação pelos links do sistema.
    document.addEventListener("click", event => {
        if (!(event.target instanceof Element)) {
            return;
        }

        const link = event.target.closest("a[href]");

        if (!link || !formularioAlterado || formularioEnviado) {
            return;
        }

        // Não interfere em novas abas ou downloads.
        if (
            link.target ||
            link.hasAttribute("download") ||
            event.ctrlKey ||
            event.shiftKey ||
            event.altKey ||
            event.metaKey
        ) {
            return;
        }

        const destino = new URL(link.href, window.location.href);

        if (destino.href === window.location.href) {
            return;
        }

        const confirmou = window.confirm(
            "Existem alterações não salvas neste cadastro.\n\n" +
            "Deseja realmente sair e descartar essas alterações?"
        );

        if (!confirmou) {
            event.preventDefault();
        } else {
            formularioEnviado = true;
        }
    });

    // Protege contra fechamento ou atualização da página.
    window.addEventListener("beforeunload", event => {
        if (formularioAlterado && !formularioEnviado) {
            event.preventDefault();
            event.returnValue = "";
        }
    });
})();
