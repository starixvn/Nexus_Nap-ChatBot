import React from 'react';
import { TextField } from '../TextField';
import { shallow, render, configure } from 'enzyme';
import Adapter from 'enzyme-adapter-react-16';

describe('TextField', () => {
    configure({ adapter: new Adapter() });

    it('when invalid - render correctly', () => {
        // Arrange
        let props = {            
        };

        // Act
        let wrapper = shallow(
            <TextField {...props} />
        );
        wrapper.instance().isShowInvalid  = jest.fn(() => { return true; });
        wrapper.instance().forceUpdate();

        // Assert
        expect(wrapper.render()).toMatchSnapshot();
    });

    it('when invalid - render correctly', () => {
        // Arrange
        let props = {            
        };

        // Act
        let wrapper = shallow(
            <TextField {...props} />
        );
        wrapper.instance().isShowInvalid = jest.fn(() => { return false; });
        wrapper.instance().forceUpdate();

        // Assert
        expect(wrapper.render()).toMatchSnapshot();
    });

    it('onChange - call setState', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new TextField(props);
        inputField.setState = jest.fn();
        inputField.callOnChange = jest.fn();
        let event = {
            target: { value: "xx" }
        };

        // Act
        inputField.onChange(event);

        // Assert
        expect(inputField.setState).toBeCalledWith(
            { value: event.target.value },
            inputField.callOnChange);
    });

    it('callOnChange - call props.onChange', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new TextField(props);
        inputField.state = { value: "ab" };
        inputField.isValid = jest.fn(() => { return true; });

        // Act
        inputField.callOnChange();

        // Assert
        expect(props.onChange).toBeCalledWith({ value: "ab", isValid: true });
    });

    it('isValid - when value Not Empty - return true', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new TextField(props);
        inputField.state = { value: '123' };

        // Act
        var result = inputField.isValid();

        // Assert
        expect(result).toBe(true);
    });

    it('isValid - when value Empty - return false', () => {
        // Arrange
        let props = { onChange: jest.fn() };
        let inputField = new TextField(props);
        inputField.state = { value: '' };

        // Act
        var result = inputField.isValid();

        // Assert
        expect(result).toBe(false);
    });

    it('isShowInvalid  - when true', () => {
        // Arrange
        let props = {
            required: true,
            isSummited: true
        };
        let inputField = new TextField(props);
        inputField.isValid = jest.fn(() => { return false; });

        // Act
        var result = inputField.isShowInvalid();

        // Assert
        expect(result).toBe(true);
    });

    it('isShowInvalid - when required false - return false', () => {
        // Arrange
        let props = {
            required: false,
            isSummited: true
        };
        let inputField = new TextField(props);
        inputField.isValid = jest.fn(() => { return false; });

        // Act
        var result = inputField.isShowInvalid();

        // Assert
        expect(result).toBe(false);
    });

    it('isShowInvalid - when isSubmmited false - return false', () => {
        // Arrange
        let props = {
            required: true,
            isSummited: false
        };
        let inputField = new TextField(props);
        inputField.isValid = jest.fn(() => { return false; });

        // Act
        var result = inputField.isShowInvalid();

        // Assert
        expect(result).toBe(false);
    });

    it('isShowInvalid - when isValid true - return false', () => {
        // Arrange
        let props = {
            required: false,
            isSummited: true
        };
        let inputField = new TextField(props);
        inputField.isValid = jest.fn(() => { return false; });

        // Act
        var result = inputField.isShowInvalid();

        // Assert
        expect(result).toBe(false);
    });
});