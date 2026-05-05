import Topbar from '@/shared/components/navbars/Topbar'
import { Outlet } from 'react-router-dom'

const AppShell = () => {
  return (
    <div className='max-w-4xl mx-auto test'>
        <Topbar/>
        <main>
            <Outlet/> 
        </main>   
    </div>
  )
}

export default AppShell