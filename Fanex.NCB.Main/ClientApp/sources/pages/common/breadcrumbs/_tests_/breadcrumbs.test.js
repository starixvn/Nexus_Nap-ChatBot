import React from 'react';
import ShallowRenderer from 'react-test-renderer/shallow'; 
import { NCBBreadcrum } from '../breadcrumbs';

describe('NCBBreadcrum', () => {
    it('NCBBreadcrum - render correctly', () => {
        // Arrange
        let props = {
            chatbotName: "a",
            pageTitle : "b"
        };

        // Act
        let dataGrid = (new ShallowRenderer()).render(<NCBBreadcrum {...props} />);

        // Assert
        expect(dataGrid).toMatchSnapshot();
    });
});