import ProtectedRoute from "@/src/components/common/ProtectedRoute"
export default function PatientAppointment() {
    return (
        <ProtectedRoute>
            <div>
                Here is Appointment Page
            </div>
        </ProtectedRoute>
    )
}