export const generateId = (length = 16): string => {
  const pool = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
  let id = ""
  for (let i = 0; i < length; i++) {
    id += pool[Math.floor(Math.random() * pool.length)]
  }
  return id
}
