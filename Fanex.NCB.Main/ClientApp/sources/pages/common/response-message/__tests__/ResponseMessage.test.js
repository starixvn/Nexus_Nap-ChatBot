import React from 'react';
import { ResponseMessage } from '../ResponseMessage';
import renderer from 'react-test-renderer';

describe("ResponseMessage", () => {
    it("render matched snapshot", () => {
        // Arrange
        let props = {
            isSuccessful: true,
            message: "Success"
        };

        // Act

        const component = renderer.create(<ResponseMessage props={props} />);
        const result = component.toJSON();

        // Assert
        expect(result).toMatchSnapshot();
    });
});