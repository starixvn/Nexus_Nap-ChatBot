jest.mock('react-redux');
jest.mock('redux');
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../../actions';

describe('connect', () => {
    let mapStateToProps, mapDispatchToProps;
    const connectMock = jest.fn();

    beforeAll(() => {
        connect.mockImplementation((mapStateToPropsFunc, mapDispatchToPropsFunc) => {
            mapStateToProps = mapStateToPropsFunc;
            mapDispatchToProps = mapDispatchToPropsFunc;
            return connectMock;
        });
        require('../DataGrid').default;
    });

    afterAll(() => {
        jest.clearAllMocks();
    });

    it('props should be mapped correctly', () => {
        // Arrange
        const expected = {
            gridData: {
                data: [{
                    notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                    title: 'title', content: 'content', unRemovable: true
                }],
                total: 1
            },
            gridParams: {
                pageIndex: 1,
                pageSize: 100
            }
        };
        const state = {
            gridData: {
                data: [{
                    notificationTemplateId: 1, notificationTypeName: 'test', notificationTemplateName: 'test123',
                    title: 'title', content: 'content', unRemovable: true
                }],
                total: 1
            },
            gridParams: {
                pageIndex: 1,
                pageSize: 100
            }
        };

        // Act
        const actual = mapStateToProps(state);

        // Assert
        expect(actual).toEqual(expected);
    });

    it('bindActionCreators should be called with correct parameter', () => {
        // Arrange
        const mockBindAction = jest.fn();
        bindActionCreators.mockImplementationOnce(mockBindAction);
        const dispatch = {};

        // Act
        mapDispatchToProps(dispatch);

        // Assert
        expect(mockBindAction).toHaveBeenCalledWith(ActionCreators, dispatch);
    });
});