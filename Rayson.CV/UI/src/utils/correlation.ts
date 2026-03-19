const USER_CORRELATION_KEY = 'rayson_cv_user_correlation_id';

export function getUserCorrelationId(): string {
  let id = localStorage.getItem(USER_CORRELATION_KEY);
  if (!id) {
    id = crypto.randomUUID();
    localStorage.setItem(USER_CORRELATION_KEY, id);
  }
  return id;
}

export function getCorrelationId(): string {
  return crypto.randomUUID();
}
