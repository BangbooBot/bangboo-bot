import Card from '@/components/RiduCard'
import { createFileRoute } from '@tanstack/react-router'

export const Route = createFileRoute('/_public/')({
  component: App,
})

function App() {
  return (
    <>
      <div className="min-h-[calc(100vh-80px)] mt-[80px] flex items-center justify-center px-8 py-12 bg-linear-to-b from-zinc-900 to-zinc-950">
        <Card />
      </div>
    </>
  )
}
