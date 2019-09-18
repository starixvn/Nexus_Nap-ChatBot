import React from 'react';
import { ResponseMessage } from '../ResponseMessage';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe("ResponseMessage", () => {
    it("render matched snapshot", () => {
        // Arrange
        let props = {
            isSuccessful: true,
            message: "Success"
        };

        // Act

        let component = (new ShallowRenderer()).render(<ResponseMessage {...props} />);

        // Assert
        expect(component).toMatchSnapshot();
    });
});