import { Suspense } from 'react'
import { DashboardSkeleton } from './DashboardSkeleton'

function DashboardContent() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-semibold">Landlord Dashboard</h1>
    </div>
  )
}

export default function LandlordDashboardPage() {
  return (
    <Suspense fallback={<DashboardSkeleton />}>
      <DashboardContent />
    </Suspense>
  )
}
