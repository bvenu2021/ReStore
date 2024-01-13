export function getCookie(key: string) {
  const b = document.cookie.match('(^|;)\\s*' + key + '\\s*=\\s*([^;]+)');

  return b ? b.pop() : '';
}

export function currencyFormat(itemPrice: number) {
  return '₹' + (itemPrice / 100).toFixed(2);
}
