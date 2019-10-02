import React from 'react';
import ShallowRenderer from 'react-test-renderer/shallow';
import ConfirmPopup from '../ConfirmPopup';

describe('ConfirmPopup', () => {
    it('ConfirmPopup when having warningIcon - render correctly', () => {
        // Arrange
        let props = {
            warningIcon: true,
            onConfirm: jest.fn()
        };

        // Act
        let dataGrid = (new ShallowRenderer()).render(<ConfirmPopup {...props} />);

        // Assert
        expect(dataGrid).toMatchSnapshot();
    });

    it('ConfirmPopup when Not having warningIcon - render correctly', () => {
        // Arrange
        let props = {
            warningIcon: false,
            onConfirm: jest.fn()
        };

        // Act
        let dataGrid = (new ShallowRenderer()).render(<ConfirmPopup {...props} />);

        // Assert
        expect(dataGrid).toMatchSnapshot();
    });
});