import { createContext, useContext, useEffect, useState } from 'react';
import type { ReactNode } from 'react';

const DESKTOP_QUERY = '(min-width: 1024px)';

interface MatchMediaContextType {
    isDesktop: boolean;
}

const MatchMediaContext = createContext<MatchMediaContextType | undefined>(undefined);

function getInitialIsDesktop(): boolean {
    if (typeof window === 'undefined') return false;
    return window.matchMedia(DESKTOP_QUERY).matches;
}

interface MatchMediaProviderProps {
    children: ReactNode;
}

export function MatchMediaProvider({ children }: MatchMediaProviderProps) {
    const [isDesktop, setIsDesktop] = useState<boolean>(getInitialIsDesktop);

    useEffect(function() {
        if (typeof window === 'undefined') return;

        const media = window.matchMedia(DESKTOP_QUERY);
        function listener() {
            setIsDesktop(media.matches);
        }

        media.addEventListener('change', listener);
        return function() { 
            media.removeEventListener('change', listener);
        };
    }, []);

    return (
        <MatchMediaContext.Provider value={{ isDesktop }}>
            {children}
        </MatchMediaContext.Provider>
    );
}

export default function useMatchMedia() {
    const context = useContext(MatchMediaContext);
    if (context === undefined) {
        throw new Error('useMatchMedia must be used within a MatchMediaProvider');
    }
    return context;
}
