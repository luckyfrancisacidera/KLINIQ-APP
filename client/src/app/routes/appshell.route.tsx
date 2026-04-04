import AppShell from "../layout/AppShell";
import { RequireAuth } from "./route.guard";

export const AppShellRoute = () => (
  <>
    <RequireAuth>
      <AppShell/>
    </RequireAuth>
  </>
)
