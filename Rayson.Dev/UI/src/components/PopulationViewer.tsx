import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config';

interface PopulationMember {
  fitness: number;
  genes: number[];
}

const PopulationViewer: React.FC = () => {
  const [populationData, setPopulationData] = useState<PopulationMember[]>([]);

  useEffect(() => {
    const fetchPopulationData = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/population_data`);
        setPopulationData(response.data);
      } catch (error) {
        console.error('Error fetching population data:', error);
      }
    };

    fetchPopulationData();
    const interval = setInterval(fetchPopulationData, 1000); // Update every 1 second

    return () => clearInterval(interval);
  }, []);

  const getColor = (genes: number[]) => {
    // Simple color mapping: use the first three genes as RGB values
    const r = Math.floor(genes[0]);
    const g = Math.floor(genes[1]);
    const b = Math.floor(genes[2]);
    return `rgb(${r}, ${g}, ${b})`;
  };

  const getBrightness = (fitness: number) => {
    // Normalize fitness to a value between 0.2 and 1
    const normalizedFitness = 0.2 + (fitness * 0.8);
    return normalizedFitness;
  };

  return (
    <div className="population-viewer">
      <h2>Population Viewer</h2>
      <div className="population-grid">
        {populationData.map((member, index) => (
          <div
            key={index}
            className="population-member"
            style={{
              backgroundColor: getColor(member.genes),
              opacity: getBrightness(member.fitness),
              width: '20px',
              height: '20px',
              margin: '2px',
            }}
          />
        ))}
      </div>
    </div>
  );
};

export default PopulationViewer;
