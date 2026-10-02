import { ReactNode } from "react"

type Props = {
    children: ReactNode
    className?: string
}

export default function AuroraSection({ children, className = "" }: Props) {
    return (
        <section className={`aurora-bg ${className}`}>
            <div className="aurora-blob aurora-blob--top-right" aria-hidden="true" />
            <div className="aurora-blob aurora-blob--bottom-left" aria-hidden="true" />
            <div className="relative">{children}</div>
        </section>
    )
}