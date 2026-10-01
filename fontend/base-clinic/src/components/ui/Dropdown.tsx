'use client'
import { useState, useEffect, useRef, ReactNode } from "react"

type DropdownProps = {
    trigger: ReactNode; // Phần tử để click mở menu (VD: Nút Avatar)
    children: ReactNode; // Các menu items bên trong
    align?: 'left' | 'right'; // Căn lề cho popup
}
export default function Dropdown({ trigger, children, align = 'left' }: DropdownProps) {
    const [isOpen, setIsOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    return (
        <div className="relative inline-block text-left" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="focus:outline-none"
            >
                {trigger}
            </button>
            {isOpen && (
                <div className={`origin-top-right absolute right-0 mt-2 w-56 rounded-md shadow-lg py-1 bg-white ring-1 ring-gray-200 focus:outline-none ${align === 'right' ? 'right-0' : 'left-0'}`}>
                    {children}
                </div>
            )}
        </div>
    );
}