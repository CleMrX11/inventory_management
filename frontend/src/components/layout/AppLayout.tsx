import { Outlet } from 'react-router-dom'
import { AppMenu } from './AppMenu'

export function AppLayout() {
  return (
    <div className="min-h-svh bg-background">
      <AppMenu />
      <main className="mx-auto max-w-6xl px-4 py-8 sm:px-6 lg:px-8">
        <Outlet />
      </main>
    </div>
  )
}
