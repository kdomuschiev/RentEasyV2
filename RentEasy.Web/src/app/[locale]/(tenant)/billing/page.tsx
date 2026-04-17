import { Suspense } from 'react'
import { BillingSkeleton } from './BillingSkeleton'

function BillingContent() {
  return (
    <div className="p-6">
      <h1 className="text-2xl font-semibold">Billing</h1>
    </div>
  )
}

export default function TenantBillingPage() {
  return (
    <Suspense fallback={<BillingSkeleton />}>
      <BillingContent />
    </Suspense>
  )
}
