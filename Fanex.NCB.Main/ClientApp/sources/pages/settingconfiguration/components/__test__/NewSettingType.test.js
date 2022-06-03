import React from 'react';
import { NewSettingType } from '../NewSettingType';
import { shallow } from 'enzyme';
import { Input} from 'reactstrap';

describe('NewSettingType', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    beforeAll(() => {
        jest.useFakeTimers();
    });

    describe('toggleHandler', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                toggleHandler: jest.fn()
            };

            // Act
            const container = shallow(<NewSettingType {...props}/>);
            container.instance().toggleHandler();

            // Assert
            expect(props.toggleHandler).toHaveBeenCalled();
        });
    });

    describe('renderForm', () => {
        const params = {
            values: {
                type: 'test',
                name: 'testing'
            },
            errors: {
                type: true,
                name: true
            },
            touched: {
                type: true,
                name: true
            },
            handleChange: jest.fn(),
            handleSubmit: jest.fn(),
            setFieldValue: jest.fn(),
            isSubmitting: jest.fn()
        };

        it('renderForm Input exactly', () => {
            // Act
            const container = shallow(<NewSettingType />);
            const renderForm = shallow(container.instance().renderForm({...params}));

            // Assert
            expect(renderForm.find(Input).length).toBe(2);
        });
    });

    describe('handleSubmit', () => {
        it('should call exactly props function when successful', async () => {
            // Arrange
            const promise = Promise.resolve({});
            const props = {
                createSettingTypeResult: { isSuccessful: true },
                createSettingType: jest.fn(() => promise),
                onCreateSuccess: jest.fn()

            };
            const values = { id: 1 };
            const setSubmitting = jest.fn();

            // Act
            const container = shallow(<NewSettingType {...props}/>);
            await container.instance().handleSubmit(values, { setSubmitting });

            jest.advanceTimersByTime(1000);

            // Assert
            expect(setSubmitting).toHaveBeenCalledWith(true);
            expect(props.createSettingType).toHaveBeenCalled();
            expect(setTimeout).toHaveBeenCalled();
            expect(props.onCreateSuccess).toHaveBeenCalled();
        });

        it('should call exactly props function when unSuccessful', async () => {
            // Arrange
            const promise = Promise.resolve({});
            const props = {
                createSettingTypeResult: { isSuccessful: false },
                createSettingType: jest.fn(() => promise),
                onCreateSuccess: jest.fn()

            };
            const values = { id: 1 };
            const setSubmitting = jest.fn();

            // Act
            const container = shallow(<NewSettingType {...props}/>);
            await container.instance().handleSubmit(values, { setSubmitting });

            // Assert
            expect(setSubmitting.mock.calls[0][0]).toBe(true);
            expect(setSubmitting.mock.calls[1][0]).toBe(false);
        });
    });
});