import React from 'react';
import ResponseMessage from '../ResponseMessage';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe("ResponseMessage", () => {
    it("render matched snapshot when success", () => {
        // Arrange
        let props = {
            isSuccessful: true,
            message: "Success"
        };

        // Act

        let component = renderer.create(<ResponseMessage {...props} />);

        // Assert
        expect(component).toMatchSnapshot();
    });

    it("render matched snapshot whenfailed", () => {
        // Arrange
        let props = {
            isSuccessful: false,
            message: "Failed"
        };

        // Act

        let component = renderer.create(<ResponseMessage {...props} />);

        // Assert
        expect(component).toMatchSnapshot();
    });
});