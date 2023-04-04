import React from "react"
import ReactDOM from "react-dom/client"
import { Provider } from "react-redux"
import "./index.css"
import { store } from "./store/store"

import "@fontsource/inter/variable.css"
import Root from "./routes/root"
import { createBrowserRouter, Router, RouterProvider } from "react-router-dom"
import HomePage from "./routes/home-page"
import ClientIndexPage from "./routes/clients/client-index-page"
import ClientDashboardPage from "./routes/clients/client-dashboard-page"
import ClientLogsPage from "./routes/clients/client-logs-page"
import ClientConsolePage from "./routes/clients/client-console-page"

const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    children: [
      {
        path: "/",
        element: <HomePage />,
      },
      {
        path: "/clients/:clientId",
        element: <ClientIndexPage />,
        children: [
          {
            path: "",
            element: <ClientDashboardPage />,
          },
          { path: "logs", element: <ClientLogsPage /> },
          {path: "console", element: <ClientConsolePage/>}
        ],
      },
    ],
  },
])

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <Provider store={store}>
      <RouterProvider router={router} />
    </Provider>
  </React.StrictMode>
)
