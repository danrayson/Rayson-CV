import React from 'react';

interface TimelineProps {
  children: React.ReactNode;
}

export const Timeline: React.FC<TimelineProps> = ({ children }) => {
  return (
    <div className="relative w-full max-w-4xl mx-auto">
      <div className="absolute left-4 top-0 bottom-0 w-0.5 bg-gradient-to-b from-transparent via-primary to-transparent md:left-1/2 md:-translate-x-1/2" />
      {children}
    </div>
  );
};

interface TimelineNodeProps {
  isFirst?: boolean;
}

export const TimelineNode: React.FC<TimelineNodeProps> = ({ isFirst }) => {
  return (
    <div className="absolute left-4 md:left-1/2 w-4 h-4 rounded-full bg-primary transform -translate-x-1/2 z-10 ring-4 ring-base-200">
      {!isFirst && (
        <div className="absolute top-full left-1/2 w-0.5 h-8 bg-primary transform -translate-x-1/2 -translate-y-1/2" />
      )}
    </div>
  );
};

export default Timeline;
