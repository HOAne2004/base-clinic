import PatientHeader from "@/src/components/layout/PatientHeader"
import PatientFooter from "@/src/components/layout/PatientFooter"

export default function PatientLayout({ children }: { children: React.ReactNode }) {
    return (
        <>
            <PatientHeader />
            {children}
            <PatientFooter />
        </>
    )
}