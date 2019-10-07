import React from 'react';
import NoDataTitle from '../NoDataTitle';
import ShallowRenderer from 'react-test-renderer/shallow';


describe('NoDataTitle', () => {
    it("render NoDataTitle correctly", () => {
        // Arrange
        let props = {            
        };

        // Act
        let result = (new ShallowRenderer()).render(<NoDataTitle {...props} />);

        // Assert
        expect(result).toMatchSnapshot();
    });
});


