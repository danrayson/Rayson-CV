const USER_CORRELATION_KEY = 'rayson_cv_user_correlation_id';
const CORRELATION_KEY = 'rayson_cv_correlation_id';

export function getUserCorrelationId(): string {
  let id = sessionStorage.getItem(USER_CORRELATION_KEY);
  if (!id) {
    id = crypto.randomUUID();
    sessionStorage.setItem(USER_CORRELATION_KEY, id);
  }
  return id;
}

export function getCorrelationId(): string {
  let id = sessionStorage.getItem(CORRELATION_KEY);
  if (!id) {
    id = crypto.randomUUID();
    sessionStorage.setItem(CORRELATION_KEY, id);
  }
  return id;
}
