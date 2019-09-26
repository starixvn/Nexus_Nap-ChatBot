import React from 'react';
import ReactDOM from 'react-dom';
import Layout from './Layout';

export function renderLayout(content) {
    ReactDOM.render(
        <Layout>
            {content}
        </Layout>, document.getElementById('root'));
}