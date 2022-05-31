import React from 'react';
import { EditMapping } from '../EditMapping';
import { shallow, mount } from 'enzyme';
import { Input } from 'reactstrap';
import ResponseMessage from '../../../common/response-message/ResponseMessage';

describe('EditMapping', () => {

    afterAll(() => {
        jest.clearAllMocks();
    });

    beforeAll(() => {
        jest.useFakeTimers();
    })

    const initProps = {
        setEditMappingResult: jest.fn(),
        getMsgContentsTypes: jest.fn(),
        getNotificationTypeRef: jest.fn(),
        jobInfo: { id: 1 },
        settingTypes: [{ id: 1}],
        editMappingPopup: { mapping: { notificationTypeID: 1 } }
    };

    describe('componentDidMount', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            container.componentDidMount();

            // Assert
            expect(props.setEditMappingResult).toHaveBeenCalled();
            expect(props.getMsgContentsTypes).toHaveBeenCalled();
            expect(props.getNotificationTypeRef).toHaveBeenCalled();
            expect(container.refs.form.setValues).toHaveBeenCalled();
        });
    });

    describe('handleNotificationTypeChange', () => {
        it('should call exactly props function', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                target: { value: 1 }
            };

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            container.handleNotificationTypeChange(mockEvent);

            // Assert
            expect(props.getNotificationTypeRef).toHaveBeenCalledWith(1);
            expect(container.refs.form.setValues).toHaveBeenCalledWith({ id: 1, name: 'test', notificationTypeId: 1 });
        });
    });

    describe('handleTemplateChange', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                target: { value: 1 }
            };

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            container.handleTemplateChange(mockEvent);

            // Assert
            expect(container.refs.form.setValues).toHaveBeenCalledWith({ id: 1, name: 'test', notificationTemplateId: 1 });
        });
    });

    describe('handleMsgContentsTypeChange', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                target: { value: 1 }
            };

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            container.handleMsgContentsTypeChange(mockEvent);

            // Assert
            expect(container.refs.form.setValues).toHaveBeenCalledWith({ id: 1, name: 'test', msgContentsType: 1 });
        });
    });

    describe('handleRequiredParamChange', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                target: { value: 1 }
            };

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            container.handleRequiredParamChange(mockEvent);

            // Assert
            expect(container.refs.form.setValues).toHaveBeenCalledWith({ id: 1, name: 'test', requiredParam: 1 });
        });
    });

    describe('handleChangeIsTest', () => {
        it('should call exactly function', () => {
            // Arrange
            const props = {...initProps};
            const mockEvent = {
                target: { value: 1 }
            };

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test', isTest: true }
                    },
                    setValues: jest.fn()
                }
            };
            container.handleChangeIsTest(mockEvent);

            // Assert
            expect(container.refs.form.setValues).toHaveBeenCalledWith({ id: 1, name: 'test', isTest: false });
        });
    });

    describe('renderNotificationTypeOption', () => {
        it('should render exactly template', () => {
            // Arrange
            const props = {...initProps};
            const item = {
                notificationTypeID: 1,
                notificationTypeName: 'test'
            };

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            const actual = container.renderNotificationTypeOption(item);

            // Assert
            expect(actual).toEqual(<option key={1} value={1}>test</option>);
        });
    });

    describe('renderNotificationTemplateOption', () => {
        it('should render exactly template', () => {
            // Arrange
            const props = {...initProps};
            const item = {
                notificationTemplateId: 1,
                notificationTemplateName: 'test'
            };

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            const actual = container.renderNotificationTemplateOption(item);

            // Assert
            expect(actual).toEqual(<option key={1} value={1}>test</option>);
        });
    });

    describe('renderMsgContentsTypeOption', () => {
        it('should render exactly template', () => {
            // Arrange
            const props = {...initProps};
            const item = {
                type: 1,
                name: 'test'
            };

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            const actual = container.renderMsgContentsTypeOption(item);

            // Assert
            expect(actual).toEqual(<option key={1} value={1}>test</option>);
        });
    });

    describe('handleSubmit', () => {
        it('should call exactly functions when success', async () => {
            // Arrange 
            const promise = Promise.resolve({ data: { code: 0, isSuccessful: true }});
            const props = {
                ...initProps,
                editMapping: jest.fn(() => promise),
                onEditSuccess: jest.fn(),
                editMappingResult: { isSuccessful: true }
            };
            const values = { id: 1, name: 'test' };
            const setSubmitting = jest.fn();

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            await container.handleSubmit(values, { setSubmitting });
            jest.advanceTimersByTime(1000);

            // Assert
            expect(setSubmitting.mock.calls[0][0]).toBe(true);
            expect(props.editMapping).toHaveBeenCalled();
            expect(setTimeout).toHaveBeenCalled();
            expect(props.onEditSuccess).toHaveBeenCalled();
        });

        it('should call exactly functions when not success', async () => {
            // Arrange
            const promise = Promise.resolve({ data: { code: 0, isSuccessful: false }});
            const props = {
                ...initProps,
                editMapping: jest.fn(() => promise),
                onEditSuccess: jest.fn(),
                editMappingResult: { isSuccessful: false }
            };
            const values = { id: 1, name: 'test' };
            const setSubmitting = jest.fn();

            // Act
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'test' }
                    },
                    setValues: jest.fn()
                }
            };
            await container.handleSubmit(values, { setSubmitting });

            // Assert
            expect(setSubmitting.mock.calls[0][0]).toBe(true);
            expect(setSubmitting.mock.calls[1][0]).toBe(false);
        });
    });

    describe('renderForm', () => {
        const props = {
            ...initProps,
            onClose: jest.fn()
        }
        const params = {
            values: {},
            errors: {},
            touched: {},
            handleChange: jest.fn(),
            handleSubmit: jest.fn(),
            setFieldValue: jest.fn(),
            isSubmitting: jest.fn()
        };

        it('should show template exactly', () => {
            // Arrange
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'Yup' }
                    },
                    setValues: jest.fn()
                }
            }
            // Act
            const renderForm = shallow(container.renderForm(params));
    
            // Assert 
            expect(renderForm.find(Input).length).toBe(4);
        });
    });

    describe('render', () => {
        it('should show template exactly', () => {
            // Arrange
            const props = {
                ...initProps,
                editMappingResult: { isSuccessful: true }
            };
            const container = mount(<EditMapping {...props}/>).instance();
            container.refs = {
                form: {
                    state: {
                        values: { id: 1, name: 'Yup' }
                    },
                    setValues: jest.fn()
                }
            }
            // Act
            const render = shallow(container.render());
    
            // Assert 
            expect(render.find(ResponseMessage).length).toBe(1);
        });
    });
});