// datetime-local trabalha em horário local sem timezone; convertemos para o
// instante UTC correto (não apenas anexando "Z") antes de enviar à API.
export function localInputParaUtcIso(valorLocal) {
  return new Date(valorLocal).toISOString();
}

// A API retorna dataHora em UTC (com "Z"). Para reexibir no input
// datetime-local, formatamos manualmente com os getters locais do Date
// (não os getUTC*), para não deslocar o horário mostrado ao usuário.
export function utcIsoParaLocalInput(dataHoraUtc) {
  const data = new Date(dataHoraUtc);
  const pad = (numero) => String(numero).padStart(2, "0");

  const ano = data.getFullYear();
  const mes = pad(data.getMonth() + 1);
  const dia = pad(data.getDate());
  const horas = pad(data.getHours());
  const minutos = pad(data.getMinutes());

  return `${ano}-${mes}-${dia}T${horas}:${minutos}`;
}

export function formatarDataHoraLocal(dataHoraUtc) {
  return new Date(dataHoraUtc).toLocaleString();
}
