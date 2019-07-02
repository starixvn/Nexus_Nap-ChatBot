import React from 'react';
import { ResponseMessage } from '../ResponseMessage';
import renderer from 'react-test-renderer';
import ShallowRenderer from 'react-test-renderer/shallow';

describe("ResponseMessage", () => {
    it("render - when responseData null - return null", () => {
        // Arrange
        let props = {
            responseData: null,
            successMsg: "successfully"
        };
        let responseMsg = new ResponseMessage(props);

        // Act
        let result = responseMsg.render();

        // Assert
        expect(result).toBe(null);
    });

    it("render - when responseData.errorCode underfine - return null", () => {
        // Arrange
        let props = {
            responseData: {},
            successMsg: "successfully"
        };
        let responseMsg = new ResponseMessage(props);

        // Act
        let result = responseMsg.render();

        // Assert
        expect(result).toBe(null);
    });

    it("render - when success is true", () => {
        // Arrange
        let props = {
            responseData: {
                errorCode: 0
            },
            successMsg: "successfully"
        };

        // Act
        let result = (new ShallowRenderer()).render(<ResponseMessage {...props} />);

        // Assert
        expect(result).toMatchSnapshot();
    });

    it("render - when success is false", () => {
        // Arrange
        let props = {
            responseData: {
                errorCode: 1,
                errorMsg: "some error"
            }            
        };

        // Act
        let result = (new ShallowRenderer()).render(<ResponseMessage {...props} />);

        // Assert
        expect(result).toMatchSnapshot();
    });
});
