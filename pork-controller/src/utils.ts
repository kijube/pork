export const generateId = (length = 16): string => {
  const pool = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
  let id = ""
  for (let i = 0; i < length; i++) {
    id += pool[Math.floor(Math.random() * pool.length)]
  }
  return id
}

type ObjectValueFilter = (key: string, value: any) => boolean

export const walkRecursively = (
  obj: any,
  filter: ObjectValueFilter | undefined = undefined,
  visited: WeakSet<any> = new WeakSet()
) => {
  if (visited.has(obj)) return {}
  visited.add(obj)

  let result: any = {}
  for (const key in obj) {
    try {
      if (filter && filter(key, obj[key])) {
        result[key] = "[filtered]"
        continue
      }
      if (typeof obj[key] === "object") {
        const child = walkRecursively(obj[key], filter, visited)
        if (Object.keys(child).length) {
          result[key] = child
        }
      } else if (Array.isArray(obj[key])) {
        const child = obj[key].map((item: any) => {
          if (typeof item === "object") {
            return walkRecursively(item, filter, visited)
          }
          return item
        })
        if (child.length) {
          result[key] = child
        }
      } else if (typeof obj[key] === "function") {
        //result[key] = "[function]"
      } else {
        result[key] = obj[key]
      }
    } catch (error) {
      // ignore
    }
  }
  return result
}
