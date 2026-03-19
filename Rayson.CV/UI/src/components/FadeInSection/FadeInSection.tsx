import React, { useEffect, useRef, useState } from 'react';

const loggedSections = new Set<string>();

interface FadeInSectionProps {
  children: React.ReactNode;
  delay?: number;
  className?: string;
  'data-section-id'?: string;
}

export const FadeInSection: React.FC<FadeInSectionProps> = ({
  children,
  delay = 0,
  className = '',
  'data-section-id': sectionId,
}) => {
  const [isVisible, setIsVisible] = useState(false);
  const ref = useRef<HTMLDivElement>(null);
  const hasEmittedRef = useRef(false);

  useEffect(() => {
    const element = ref.current;
    if (!element) return;

    const visibilityObserver = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting && !hasEmittedRef.current) {
          hasEmittedRef.current = true;
          setTimeout(() => setIsVisible(true), delay);
        }

        const sectionId = entry.target.getAttribute('data-section-id');
        if (sectionId && !loggedSections.has(sectionId)) {
          loggedSections.add(sectionId);
          window.dispatchEvent(new CustomEvent('section-visible', {
            detail: { sectionId }
          }));
        }
      },
      { threshold: 0.1, rootMargin: '0px 0px -50px 0px' }
    );

    visibilityObserver.observe(element);

    return () => {
      visibilityObserver.disconnect();
    };
  }, [delay]);

  return (
    <div
      ref={ref}
      data-section-id={sectionId}
      className={`transition-all duration-700 ease-out transform ${
        isVisible ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'
      } ${className}`}
    >
      {children}
    </div>
  );
};

export default FadeInSection;
