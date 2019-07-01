import React from 'react';
import Labels from '../Labels';
import './NoDataTitle.scss';

const noDataTitle = () => {
    return (
        <p className="no-info-row center-align">{Labels.NoInfo}</p>
    );
};

export default noDataTitle;