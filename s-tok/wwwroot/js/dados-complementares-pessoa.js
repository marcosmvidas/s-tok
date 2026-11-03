document.addEventListener("click", async (event) => {
    const button = event.target.closest("[data-modal-salvar]");
    if (!button) return;

    const content = button.closest(".modal-content");
    const modal = button.closest(".modal");
    const errorBox = content.querySelector("[data-modal-erro]");
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    errorBox.classList.add("d-none");
    errorBox.textContent = "";

    const data = new FormData();
    content.querySelectorAll("input[name], select[name], textarea[name]").forEach((field) => {
        if (field.type === "checkbox") {
            data.append(field.name, field.checked ? "true" : "false");
        } else {
            data.append(field.name, field.value);
        }
    });

    if (token) data.append("__RequestVerificationToken", token);

    button.disabled = true;
    const originalText = button.textContent;
    button.textContent = "Salvando...";

    try {
        const response = await fetch(content.dataset.modalEndpoint, {
            method: "POST",
            body: data,
            headers: token ? { "RequestVerificationToken": token } : {}
        });

        const result = await response.json();
        if (!response.ok || !result.sucesso) {
            const messages = result.erros || [result.mensagem || "Não foi possível salvar os dados."];
            errorBox.textContent = messages.join(" ");
            errorBox.classList.remove("d-none");
            return;
        }

        bootstrap.Modal.getOrCreateInstance(modal).hide();
        window.location.reload();
    } catch {
        errorBox.textContent = "Falha de comunicação. Verifique sua conexão e tente novamente.";
        errorBox.classList.remove("d-none");
    } finally {
        button.disabled = false;
        button.textContent = originalText;
    }
});
