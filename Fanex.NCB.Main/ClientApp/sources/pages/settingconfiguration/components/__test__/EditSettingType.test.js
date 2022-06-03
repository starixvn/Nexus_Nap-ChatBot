import React from 'react';
import { EditNotificationType } from '../EditSettingType';
import { shallow } from 'enzyme';
import { Input} from 'reactstrap';

describe('EditNotificationType', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    beforeAll(() => {
        jest.useFakeTimers();
    });

    const initProps = {
        editSettingTypePopup: {
            settingType: { type: 'test', name: 'testing' }
        }
    };

    describe('toggleHandler', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                toggleHandler: jest.fn()
            };

            // Act
            const container = shallow(<EditNotificationType {...props}/>);
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
            // Arrange
            const props = {...initProps};

            // Act
            const container = shallow(<EditNotificationType {...props}/>);
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
                ...initProps,
                editSettingTypeResult: { isSuccessful: true },
                editSettingType: jest.fn(() => promise),
                onEditSuccess: jest.fn()

            };
            const values = { id: 1 };
            const setSubmitting = jest.fn();

            // Act
            const container = shallow(<EditNotificationType {...props}/>);
            await container.instance().handleSubmit(values, { setSubmitting });

            jest.advanceTimersByTime(1000);

            // Assert
            expect(setSubmitting).toHaveBeenCalledWith(true);
            expect(props.editSettingType).toHaveBeenCalled();
            expect(setTimeout).toHaveBeenCalled();
            expect(props.onEditSuccess).toHaveBeenCalled();
        });

        it('should call exactly props function when unSuccessful', async () => {
            // Arrange
            const promise = Promise.resolve({});
            const props = {
                ...initProps,
                editSettingTypeResult: { isSuccessful: false },
                editSettingType: jest.fn(() => promise),
                onEditSuccess: jest.fn()

            };
            const values = { id: 1 };
            const setSubmitting = jest.fn();

            // Act
            const container = shallow(<EditNotificationType {...props}/>);
            await container.instance().handleSubmit(values, { setSubmitting });

            // Assert
            expect(setSubmitting.mock.calls[0][0]).toBe(true);
            expect(setSubmitting.mock.calls[1][0]).toBe(false);
        });
    });

    describe('handleClose', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {
                ...initProps,
                setEditSettingTypeResult: jest.fn(),
                setEditSettingTypePopup: jest.fn()
            };

            // Act
            const container = shallow(<EditNotificationType {...props}/>);
            container.instance().handleClose();

            // Assert
            expect(props.setEditSettingTypeResult).toHaveBeenCalledWith(null);
            expect(props.setEditSettingTypePopup).toHaveBeenCalledWith({ show: false });
        });
    });
});