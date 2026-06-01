import { Package } from 'lucide-react'
import { NavLink } from 'react-router-dom'
import { cn } from '@/lib/utils'

export function AppMenu() {
  return (
    <header className="border-b bg-background">
      <div className="mx-auto flex h-14 max-w-6xl items-center gap-6 px-4 sm:px-6 lg:px-8">
        <div className="flex items-center gap-2 font-semibold">
          <Package className="h-5 w-5" aria-hidden="true" />
          <span>Inventory Management</span>
        </div>

        <nav className="flex items-center gap-4 text-sm">
          <NavLink
            to="/articles"
            className={({ isActive }) =>
              cn(
                'transition-colors hover:text-foreground',
                isActive ? 'font-medium text-foreground' : 'text-muted-foreground',
              )
            }
          >
            Articles
          </NavLink>
        </nav>
      </div>
    </header>
  )
}
