'use client'

import { ReactNode, useEffect } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faXmark } from "@fortawesome/free-solid-svg-icons";

type ModalProps = {
    show: boolean,
    onHide: () => void,
    title: string,
    children: ReactNode,
    footer?: ReactNode,
    keyboard?: boolean,
    backdrop?: 'static' | true,
};

export default function Modal({
    show,
    onHide,
    title,
    children,
    footer,
    keyboard = true,
    backdrop = true,
}: ModalProps) {
    // Escape key
    useEffect(() => {
        if (!show || !keyboard) return;
        const handleEscape = (e: KeyboardEvent) => {
            if (e.key === "Escape") onHide();
        };
        document.addEventListener("keydown", handleEscape);
        return () => document.removeEventListener("keydown", handleEscape);
    }, [show, keyboard, onHide]);

    // Lock body scroll
    useEffect(() => {
        if (show) {
            document.body.style.overflow = "hidden";
        } else {
            document.body.style.overflow = "";
        }
        return () => {
            document.body.style.overflow = "";
        };
    }, [show]);

    if (!show) return null;

    const handleBackdropClick = () => {
        if (backdrop === "static") return;
        onHide();
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
            {/* Overlay — only this layer gets blurred */}
            <div
                className="absolute inset-0 bg-black/60 backdrop-blur-sm"
                onClick={handleBackdropClick}
                aria-hidden="true"
            />

            {/* Modal panel — floats above overlay, NOT blurred */}
            <div className="relative z-10 w-full max-w-lg overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-2xl">
                {/* Header */}
                <div className="relative border-b border-gray-100 px-6 py-4">
                    <button
                        onClick={onHide}
                        aria-label="Đóng"
                        className="absolute right-6 top-4 flex h-8 w-8 items-center justify-center rounded-lg text-gray-400 transition-colors hover:bg-gray-100 hover:text-gray-700"
                    >
                        <FontAwesomeIcon icon={faXmark} className="h-4 w-4" />
                    </button>
                    <h2 className="text-center text-xl font-bold text-primary">{title}</h2>
                </div>

                {/* Body */}
                <div className="px-6 py-5">{children}</div>

                {/* Footer */}
                {footer && (
                    <div className="flex items-center justify-end gap-3 border-t border-gray-100 bg-gray-50 px-6 py-4">
                        {footer}
                    </div>
                )}
            </div>
        </div>
    );
}