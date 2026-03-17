import React, { useEffect, useRef, useState } from 'react';

interface TimelineCardProps {
  children: React.ReactNode;
  delay?: number;
  size?: 'normal' | 'small';
}

export const TimelineCard: React.FC<TimelineCardProps> = ({ 
  children, 
  delay = 0,
  size = 'normal'
}) => {
  const [isVisible, setIsVisible] = useState(false);
  const cardRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          setTimeout(() => {
            setIsVisible(true);
          }, delay);
          observer.unobserve(entry.target);
        }
      },
      {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px',
      }
    );

    if (cardRef.current) {
      observer.observe(cardRef.current);
    }

    return () => {
      if (cardRef.current) {
        observer.unobserve(cardRef.current);
      }
    };
  }, [delay]);

  return (
    <div
      ref={cardRef}
      className={`
        timeline-card
        w-full
        mb-1
        transition-all
        duration-700
        ease-out
        transform
        ${isVisible ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}
        ${size === 'small' ? 'p-1' : 'p-6'}
      `}
    >
      {children}
    </div>
  );
};

export default TimelineCard;
