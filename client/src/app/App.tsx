import { RouterProvider } from "react-router-dom"
import { AuthProvider } from "./providers/AuthProviders"
import { router } from "./routes/index"

const App = () => (
  <AuthProvider>
    <RouterProvider router={router}></RouterProvider>
  </AuthProvider>
)

export default App
