// DriveNow - scripts do frontend

// confirmacao generica antes de excluir
function confirmarExclusao(mensagem) {
    return confirm(mensagem || "Tem certeza que deseja excluir?");
}

// calcula o valor estimado na tela de nova locacao
function calcularTotal() {
    var campoRetirada  = document.getElementById("DataRetirada");
    var campoDevolucao = document.getElementById("DataDevolucao");
    var boxEstimativa  = document.getElementById("estimativa");
    var textoEstimativa = document.getElementById("textoEstimativa");

    if (!campoRetirada || !campoDevolucao || !boxEstimativa) return;

    var dataRet = campoRetirada.value;
    var dataDev = campoDevolucao.value;

    if (!dataRet || !dataDev) {
        boxEstimativa.style.display = "none";
        return;
    }

    var d1   = new Date(dataRet);
    var d2   = new Date(dataDev);
    var dias = Math.round((d2 - d1) / (1000 * 60 * 60 * 24));

    if (dias <= 0) {
        boxEstimativa.style.display = "block";
        textoEstimativa.textContent = "A data de devolucao deve ser apos a retirada.";
        boxEstimativa.style.background = "#fdecea";
        boxEstimativa.style.borderColor = "#e74c3c";
        textoEstimativa.style.color = "#721c24";
        return;
    }

    var selectVeiculo = document.getElementById("IdVeiculo");
    var opcaoSelecionada = selectVeiculo.options[selectVeiculo.selectedIndex];
    var diaria = parseFloat(opcaoSelecionada.getAttribute("data-diaria")) || 0;

    boxEstimativa.style.background   = "#e8f5e9";
    boxEstimativa.style.borderColor  = "#27ae60";
    textoEstimativa.style.color      = "#1e5631";
    boxEstimativa.style.display      = "block";

    if (diaria > 0) {
        var total = dias * diaria;
        var totalFormatado = total.toFixed(2).replace(".", ",");
        textoEstimativa.textContent = "Estimativa: " + dias + " dia(s) x R$ " +
            diaria.toFixed(2).replace(".", ",") + " = R$ " + totalFormatado;
    } else {
        textoEstimativa.textContent = "Selecione um veiculo para ver o valor estimado.";
    }
}

// mascara simples de CPF
function mascaraCpf(campo) {
    var v = campo.value.replace(/\D/g, "");
    if (v.length > 11) v = v.slice(0, 11);
    v = v.replace(/(\d{3})(\d)/, "$1.$2");
    v = v.replace(/(\d{3})(\d)/, "$1.$2");
    v = v.replace(/(\d{3})(\d{1,2})$/, "$1-$2");
    campo.value = v;
}
